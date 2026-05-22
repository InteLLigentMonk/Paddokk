export type Era = "Classic" | "Youngtimer" | "Modern classic" | "Contemporary";

export function eraFromYear(
  year: number | string | null | undefined,
): Era | null {
  const y = Number(year);
  if (!y || isNaN(y)) return null;
  if (y < 1985) return "Classic";
  if (y <= 1999) return "Youngtimer";
  if (y <= 2014) return "Modern classic";
  return "Contemporary";
}
