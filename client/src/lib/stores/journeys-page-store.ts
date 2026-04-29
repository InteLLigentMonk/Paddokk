import { Store } from "@tanstack/store";

interface JourneysPageState {
  modals: {
    createJourney: boolean;
    editJourney: {
      open: boolean;
      journeyId: number | null;
    };
    deleteJourney: {
      open: boolean;
      journeyId: number | null;
    };
  };
}

export const journeysPageStore = new Store<JourneysPageState>({
  modals: {
    createJourney: false,
    editJourney: {
      open: false,
      journeyId: null,
    },
    deleteJourney: {
      open: false,
      journeyId: null,
    },
  },
});

export function openCreateJourneyModal() {
  journeysPageStore.setState((state) => ({
    ...state,
    modals: { ...state.modals, createJourney: true },
  }));
}

export function closeCreateJourneyModal() {
  journeysPageStore.setState((state) => ({
    ...state,
    modals: { ...state.modals, createJourney: false },
  }));
}

export function openEditJourneyModal(journeyId: number) {
  journeysPageStore.setState((state) => ({
    ...state,
    modals: {
      ...state.modals,
      editJourney: { open: true, journeyId },
    },
  }));
}

export function closeEditJourneyModal() {
  journeysPageStore.setState((state) => ({
    ...state,
    modals: {
      ...state.modals,
      editJourney: { open: false, journeyId: null },
    },
  }));
}

export function openDeleteJourneyConfirm(journeyId: number) {
  journeysPageStore.setState((state) => ({
    ...state,
    modals: {
      ...state.modals,
      deleteJourney: { open: true, journeyId },
    },
  }));
}

export function closeDeleteJourneyConfirm() {
  journeysPageStore.setState((state) => ({
    ...state,
    modals: {
      ...state.modals,
      deleteJourney: { open: false, journeyId: null },
    },
  }));
}
