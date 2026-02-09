import { Button, Stack } from '@mantine/core'
import { useNotifications } from '@/integrations/mantine'
import { useMutation } from '@tanstack/react-query'

export function NotificationExamples() {
  const notifications = useNotifications()

  const mutation = useMutation({
    mutationFn: async () => {
      throw new Error('This is a test error from a mutation')
    },
  })

  const successMutation = useMutation({
    mutationFn: async () => {
      await new Promise((resolve) => setTimeout(resolve, 500))
      return { success: true }
    },
    onSuccess: () => {
      notifications.success({ message: 'Operation completed successfully!' })
    },
  })

  return (
    <Stack gap="md">
      <Button
        onClick={() =>
          notifications.success({ message: 'This is a success notification' })
        }
      >
        Show Success Notification
      </Button>

      <Button
        onClick={() =>
          notifications.error({ message: 'This is an error notification' })
        }
        color="red"
      >
        Show Error Notification
      </Button>

      <Button
        onClick={() =>
          notifications.warning({ message: 'This is a warning notification' })
        }
        color="yellow"
      >
        Show Warning Notification
      </Button>

      <Button
        onClick={() =>
          notifications.info({ message: 'This is an info notification' })
        }
        color="blue"
      >
        Show Info Notification
      </Button>

      <Button
        onClick={() =>
          notifications.success({
            title: 'Custom Title',
            message: 'This notification has a custom title',
            autoClose: 10000,
          })
        }
      >
        Show Custom Notification
      </Button>

      <Button onClick={() => mutation.mutate()} color="red">
        Test Automatic Error Handling (Mutation)
      </Button>

      <Button onClick={() => successMutation.mutate()} color="green">
        Test Success with Mutation
      </Button>
    </Stack>
  )
}
