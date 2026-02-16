import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { Container, Title } from '@mantine/core'
import { CarForm } from '@/components/cars/car-form'

export const Route = createFileRoute('/_app/cars/new')({
  component: AddCarPage,
})

function AddCarPage() {
  const navigate = useNavigate()

  return (
    <Container size="md" py="xl">
      <Title order={1} mb="xl">
        Add New Car
      </Title>
      <CarForm
        onSuccess={() => navigate({ to: '/cars' })}
        onCancel={() => navigate({ to: '/cars' })}
      />
    </Container>
  )
}
