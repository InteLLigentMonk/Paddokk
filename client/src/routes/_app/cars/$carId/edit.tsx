import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { Container, Title, Loader, Center, Alert } from '@mantine/core'
import { useGetApiUsersMeCarsCarId } from '@/generated/api/user-cars/user-cars'
import { CarForm } from '@/components/cars/car-form'
import { AlertCircle } from 'lucide-react'

export const Route = createFileRoute('/_app/cars/$carId/edit')({
  component: EditCarPage,
})

function EditCarPage() {
  const { carId } = Route.useParams()
  const navigate = useNavigate()
  const { data, isLoading, error } = useGetApiUsersMeCarsCarId(Number(carId))

  if (isLoading) {
    return (
      <Container size="md" py="xl">
        <Center>
          <Loader />
        </Center>
      </Container>
    )
  }

  if (error || !data?.data) {
    return (
      <Container size="md" py="xl">
        <Alert icon={<AlertCircle size={16} />} title="Error" color="red">
          Car not found or you don't have permission to edit it.
        </Alert>
      </Container>
    )
  }

  return (
    <Container size="md" py="xl">
      <Title order={1} mb="xl">
        Edit Car
      </Title>
      <CarForm
        initialValues={data.data}
        carId={Number(carId)}
        onSuccess={() => navigate({ to: '/cars' })}
        onCancel={() => navigate({ to: '/cars' })}
      />
    </Container>
  )
}
