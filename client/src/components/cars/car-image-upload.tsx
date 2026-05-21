import { useEffect, useState } from 'react'
import { Dropzone, IMAGE_MIME_TYPE } from '@mantine/dropzone'
import { Box, CloseButton, Group, Image, Stack, Text, rem } from '@mantine/core'
import { Image as ImageIcon, Upload, X } from 'lucide-react'
import { useNotifications } from '@/integrations/mantine'

interface CarImageUploadProps {
  value?: File | string
  onChange: (file: File | null) => void
  disabled?: boolean
}

export function CarImageUpload({ value, onChange, disabled }: CarImageUploadProps) {
  const notifications = useNotifications()
  const [preview, setPreview] = useState<string | null>(
    typeof value === 'string' ? value : null
  )

  useEffect(() => {
    if (value instanceof File) {
      const reader = new FileReader()
      reader.onload = () => setPreview(reader.result as string)
      reader.readAsDataURL(value)
    } else if (typeof value === 'string') {
      setPreview(value)
    } else {
      setPreview(null)
    }
  }, [value])

  return (
    <Stack gap="sm">
      {preview ? (
        <Box style={{ position: 'relative' }}>
          <Image src={preview} h={200} radius="md" alt="Car preview" fit="cover" />
          {!disabled && (
            <CloseButton
              onClick={() => {
                onChange(null)
                setPreview(null)
              }}
              style={{
                position: 'absolute',
                top: 8,
                right: 8,
                backgroundColor: 'var(--mantine-color-body)',
              }}
              aria-label="Remove image"
            />
          )}
        </Box>
      ) : (
        <Dropzone
          onDrop={(files) => onChange(files[0])}
          onReject={() =>
            notifications.error({ message: 'Invalid image file. Please upload an image under 5MB.' })
          }
          maxSize={5 * 1024 * 1024}
          accept={IMAGE_MIME_TYPE}
          disabled={disabled}
        >
          <Group
            justify="center"
            gap="xl"
            style={{ minHeight: rem(200), pointerEvents: 'none' }}
          >
            <Box component="span" darkHidden>
              <Dropzone.Accept>
                <Upload size={52} style={{ color: 'var(--mantine-color-blue-6)' }} />
              </Dropzone.Accept>
              <Dropzone.Reject>
                <X size={52} style={{ color: 'var(--mantine-color-red-6)' }} />
              </Dropzone.Reject>
              <Dropzone.Idle>
                <ImageIcon size={52} style={{ color: 'var(--mantine-color-dimmed)' }} />
              </Dropzone.Idle>
            </Box>

            <Box component="span" lightHidden>
              <Dropzone.Accept>
                <Upload size={52} style={{ color: 'var(--mantine-color-blue-4)' }} />
              </Dropzone.Accept>
              <Dropzone.Reject>
                <X size={52} style={{ color: 'var(--mantine-color-red-4)' }} />
              </Dropzone.Reject>
              <Dropzone.Idle>
                <ImageIcon size={52} style={{ color: 'var(--mantine-color-dimmed)' }} />
              </Dropzone.Idle>
            </Box>

            <div>
              <Text size="xl" inline>
                Drag image here or click to select
              </Text>
              <Text size="sm" c="dimmed" inline mt={7}>
                Upload a photo of your car (max 5MB)
              </Text>
            </div>
          </Group>
        </Dropzone>
      )}
    </Stack>
  )
}
