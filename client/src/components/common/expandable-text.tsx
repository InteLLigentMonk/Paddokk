import { useState } from "react";
import { Stack, Text, Collapse, ActionIcon, Center, Box } from "@mantine/core";
import { ChevronDown } from "lucide-react";

interface ExpandableTextProps {
  text: string;
  maxLines?: number;
  charsPerLine?: number;
  size?: string;
  c?: string;
}

export function ExpandableText({
  text,
  maxLines = 3,
  charsPerLine = 70,
  size,
  c,
}: ExpandableTextProps) {
  const [expanded, setExpanded] = useState(false);

  const lines = text.split("\n");
  const hasMoreLines = lines.length > maxLines;
  const charLimit = maxLines * charsPerLine;
  const hasMoreChars = !hasMoreLines && text.length > charLimit;
  const isLong = hasMoreLines || hasMoreChars;

  const previewText = hasMoreLines
    ? lines.slice(0, maxLines).join("\n")
    : hasMoreChars
      ? text.slice(0, charLimit)
      : text;

  const restText = hasMoreLines
    ? lines.slice(maxLines).join("\n")
    : text.slice(charLimit);

  const toggle = () => setExpanded((v) => !v);

  return (
    <Stack gap={4}>
      <Box
        onClick={isLong ? toggle : undefined}
        style={isLong ? { cursor: "pointer" } : undefined}
      >
        <Text size={size} c={c} style={{ whiteSpace: "pre-wrap", wordBreak: "break-word" }}>
          {previewText}
          {!expanded && isLong && "…"}
        </Text>
        <Collapse in={expanded}>
          <Text size={size} c={c} style={{ whiteSpace: "pre-wrap", wordBreak: "break-word" }}>
            {restText}
          </Text>
        </Collapse>
      </Box>

      {isLong && (
        <Center>
          <ActionIcon
            variant="transparent"
            size="sm"
            onClick={toggle}
            aria-label={expanded ? "Visa mindre" : "Visa mer"}
            style={{
              transform: expanded ? "rotate(180deg)" : "rotate(0deg)",
              transition: "transform 200ms ease",
            }}
          >
            <ChevronDown size={16} />
          </ActionIcon>
        </Center>
      )}
    </Stack>
  );
}
