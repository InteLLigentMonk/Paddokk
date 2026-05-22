import { Store } from "@tanstack/store";

export type SortOption =
  | "newest"
  | "oldest"
  | "name-asc"
  | "name-desc"
  | "year-new"
  | "year-old"
  | "journeys";

interface CarsPageState {
  sortBy: SortOption;
  modals: {
    addCar: boolean;
    editCar: {
      open: boolean;
      carId: number | null;
    };
    deleteCar: {
      open: boolean;
      carId: number | null;
    };
  };
}

export const carsPageStore = new Store<CarsPageState>({
  sortBy: "newest",
  modals: {
    addCar: false,
    editCar: {
      open: false,
      carId: null,
    },
    deleteCar: {
      open: false,
      carId: null,
    },
  },
});

// Action creators
export function setSortBy(sortBy: SortOption) {
  carsPageStore.setState((state) => ({
    ...state,
    sortBy,
  }));
}

export function openAddCarModal() {
  carsPageStore.setState((state) => ({
    ...state,
    modals: {
      ...state.modals,
      addCar: true,
    },
  }));
}

export function closeAddCarModal() {
  carsPageStore.setState((state) => ({
    ...state,
    modals: {
      ...state.modals,
      addCar: false,
    },
  }));
}

export function openEditCarModal(carId: number) {
  carsPageStore.setState((state) => ({
    ...state,
    modals: {
      ...state.modals,
      editCar: {
        open: true,
        carId,
      },
    },
  }));
}

export function closeEditCarModal() {
  carsPageStore.setState((state) => ({
    ...state,
    modals: {
      ...state.modals,
      editCar: {
        open: false,
        carId: null,
      },
    },
  }));
}

export function openDeleteCarConfirm(carId: number) {
  carsPageStore.setState((state) => ({
    ...state,
    modals: {
      ...state.modals,
      deleteCar: {
        open: true,
        carId,
      },
    },
  }));
}

export function closeDeleteCarConfirm() {
  carsPageStore.setState((state) => ({
    ...state,
    modals: {
      ...state.modals,
      deleteCar: {
        open: false,
        carId: null,
      },
    },
  }));
}
