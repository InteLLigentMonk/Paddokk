import { useGetApiUsersMeCarsCanAdd } from '@/generated/api/user-cars/user-cars'

export function useCanAddCar() {
  const { data, isLoading } = useGetApiUsersMeCarsCanAdd()

  return {
    canAdd: data?.status === 200,
    isLoading,
  }
}
