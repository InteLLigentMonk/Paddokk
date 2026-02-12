import { notify } from "../mantine";

export function createQueryErrorHandler() {
  return (error: Error) => {
    if (error.name === "AbortError") {
      return;
    }

    notify.error({
      message: error.message || "An error occurred while fetching data",
    });
  };
}

export function createMutationErrorHandler() {
  return (error: Error) => {
    if (error.name === "AbortError") {
      return;
    }

    notify.error({
      message: error.message || "An error occurred while saving data",
    });
  };
}
