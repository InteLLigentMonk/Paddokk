import { useState } from "react"
import { Button, Group, Modal, ScrollArea, Select, Stack, Text, TextInput } from "@mantine/core"
import { useStore } from "@tanstack/react-store"
import { useForm } from "@tanstack/react-form"
import { useQuery, useQueryClient } from "@tanstack/react-query"
import { journeysPageStore, closeEditJourneyModal } from "@/lib/stores/journeys-page-store"
import { updateJourneyFn, getUserJourneysFn } from "@/lib/api/user-journeys.server"
import { userJourneysUploadJourneyCoverImage } from "@/generated/api/user-journeys/user-journeys"
import { RichTextEditor } from "@/components/shared/rich-text-editor"
import { CoverImageDropzone } from "@/components/shared/cover-image-dropzone"
import { useNotifications } from "@/integrations/mantine"
import type { JourneyDto } from "@/generated/api/schemas"

const JOURNEY_CATEGORIES = [
  { value: "1", label: "Build & Mods" },
  { value: "2", label: "Restoration" },
  { value: "3", label: "Racing" },
  { value: "4", label: "Adventures" },
  { value: "5", label: "Ownership" },
]

const JOURNEY_STATUSES = [
  { value: "1", label: "Active" },
  { value: "2", label: "Completed" },
  { value: "3", label: "Parked" },
  { value: "4", label: "Archived" },
]

interface EditJourneyFormProps {
  journey: JourneyDto
  onClose: () => void
}

function EditJourneyForm({ journey, onClose }: EditJourneyFormProps) {
  const queryClient = useQueryClient()
  const notifications = useNotifications()

  const [description, setDescription] = useState(journey.description ?? "")
  const [coverFile, setCoverFile] = useState<File | null>(null)
  const [coverPreviewUrl, setCoverPreviewUrl] = useState<string | null>(journey.primaryImageUrl ?? null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const journeyId = Number(journey.id)

  const form = useForm({
    defaultValues: {
      title: journey.title,
      category: String(journey.category),
      status: String(journey.status),
    },
    onSubmit: async ({ value }) => {
      setIsSubmitting(true)
      try {
        await updateJourneyFn({
          data: {
            journeyId,
            title: value.title,
            category: Number(value.category),
            status: Number(value.status),
            description: description || null,
          },
        })

        if (coverFile) {
          await userJourneysUploadJourneyCoverImage(journeyId, { file: coverFile })
        }

        queryClient.invalidateQueries({ queryKey: ["user-journeys"] })
        queryClient.invalidateQueries({ queryKey: ["journey-detail", journeyId] })
        notifications.success({ message: "Journey updated!" })
        handleClose()
      } catch {
        notifications.error({ message: "Failed to update journey" })
        setIsSubmitting(false)
      }
    },
  })

  const handleClose = () => {
    if (coverFile && coverPreviewUrl && coverPreviewUrl !== (journey.primaryImageUrl ?? null)) {
      URL.revokeObjectURL(coverPreviewUrl)
    }
    setCoverFile(null)
    setIsSubmitting(false)
    onClose()
  }

  const handleCoverChange = (file: File | null, previewUrl: string | null) => {
    if (coverFile && coverPreviewUrl && coverPreviewUrl !== (journey.primaryImageUrl ?? null)) {
      URL.revokeObjectURL(coverPreviewUrl)
    }
    setCoverFile(file)
    setCoverPreviewUrl(previewUrl)
  }

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault()
        form.handleSubmit()
      }}
    >
      <Stack gap="md">
        <CoverImageDropzone previewUrl={coverPreviewUrl} onChange={handleCoverChange} />

        <form.Field
          name="title"
          validators={{
            onChange: ({ value }) => {
              if (!value?.trim()) return "Title is required"
              if (value.trim().length < 3) return "Title must be at least 3 characters"
              return undefined
            },
          }}
        >
          {(field) => (
            <TextInput
              label="Title"
              placeholder="Give your journey a name"
              value={field.state.value}
              onChange={(e) => field.handleChange(e.target.value)}
              error={field.state.meta.isTouched ? field.state.meta.errors.join(", ") : undefined}
              required
            />
          )}
        </form.Field>

        <form.Field
          name="category"
          validators={{
            onChange: ({ value }) => (!value ? "Category is required" : undefined),
          }}
        >
          {(field) => (
            <Select
              label="Category"
              placeholder="Select category"
              data={JOURNEY_CATEGORIES}
              value={field.state.value || null}
              onChange={(value) => field.handleChange(value ?? "")}
              error={field.state.meta.isTouched ? field.state.meta.errors.join(", ") : undefined}
              required
            />
          )}
        </form.Field>

        <form.Field name="status">
          {(field) => (
            <Select
              label="Status"
              data={JOURNEY_STATUSES}
              value={field.state.value || null}
              onChange={(value) => field.handleChange(value ?? "")}
            />
          )}
        </form.Field>

        <div>
          <Text size="sm" fw={500} mb={4}>
            Description (optional)
          </Text>
          <RichTextEditor content={description} onChange={setDescription} />
        </div>

        <Group justify="flex-end" mt="sm">
          <Button variant="subtle" onClick={handleClose} disabled={isSubmitting}>
            Cancel
          </Button>
          <Button type="submit" loading={isSubmitting}>
            Save changes
          </Button>
        </Group>
      </Stack>
    </form>
  )
}

export function EditJourneyModal() {
  const editState = useStore(journeysPageStore, (state) => state.modals.editJourney)

  const { data: journeys } = useQuery({
    queryKey: ["user-journeys"],
    queryFn: () => getUserJourneysFn(),
    enabled: editState.open,
  })

  const journey = journeys?.find((j) => Number(j.id) === editState.journeyId)

  return (
    <Modal
      opened={editState.open}
      onClose={closeEditJourneyModal}
      title="Edit journey"
      centered
      size="lg"
      scrollAreaComponent={ScrollArea.Autosize}
    >
      {journey ? (
        <EditJourneyForm key={journey.id} journey={journey} onClose={closeEditJourneyModal} />
      ) : (
        <Stack gap="md">
          <Text c="dimmed" size="sm">
            Loading...
          </Text>
        </Stack>
      )}
    </Modal>
  )
}
