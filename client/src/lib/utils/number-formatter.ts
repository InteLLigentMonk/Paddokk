export function formatNumber(num: number | string | undefined | null): string {
  if (num === null || num === undefined) return "—";
  if (typeof num === "string") {
    const parsed = Number(num);
    if (isNaN(parsed)) return num;
    num = parsed;
  }
  if (num >= 1_000_000_000) {
    return (num / 1_000_000_000).toFixed(1).replace(/\.0$/, "") + "B";
  }
  if (num >= 1_000_000) {
    return (num / 1_000_000).toFixed(1).replace(/\.0$/, "") + "M";
  }
  if (num >= 1_000) {
    return (num / 1_000).toFixed(1).replace(/\.0$/, "") + "K";
  }
  return num.toString();
}
