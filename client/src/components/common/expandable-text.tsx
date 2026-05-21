import { useEffect, useRef, useState } from "react";
import { Anchor, Stack, Text } from "@mantine/core";

interface ExpandableTextProps {
  text: string;
  maxLines?: number;
  size?: string;
  c?: string;
}

export function ExpandableText({
  text,
  maxLines = 3,
  size,
  c,
}: ExpandableTextProps) {
  const [expanded, setExpanded] = useState(false);
  const [clamped, setClamped] = useState(false);
  const ref = useRef<HTMLParagraphElement>(null);
  const expandedRef = useRef(false);

  expandedRef.current = expanded;

  useEffect(() => {
    const el = ref.current;
    if (!el) return;

    const check = () => {
      if (expandedRef.current) return;
      if (el.clientHeight === 0) return;
      setClamped(el.scrollHeight > el.clientHeight);
    };

    check();
    const ro = new ResizeObserver(check);
    ro.observe(el);
    return () => ro.disconnect();
  }, [text]);

  return (
    <Stack gap={2}>
      <Text
        ref={ref}
        size={size}
        c={c}
        style={{
          whiteSpace: "pre-wrap",
          wordBreak: "break-word",
          overflow: "hidden",
          display: "-webkit-box",
          WebkitLineClamp: expanded ? "unset" : maxLines,
          WebkitBoxOrient: "vertical",
        }}
      >
        {text}
      </Text>
      {clamped && (
        <Anchor
          size="xs"
          onClick={() => setExpanded((v) => !v)}
          style={{ cursor: "pointer" }}
        >
          {expanded ? "Visa mindre" : "Visa mer"}
        </Anchor>
      )}
    </Stack>
  );
}
