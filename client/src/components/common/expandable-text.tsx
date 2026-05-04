import { useState, useRef, useEffect } from "react";
import { Stack, Text, Collapse, ActionIcon, Center, Box } from "@mantine/core";
import { ChevronDown } from "lucide-react";

interface ExpandableTextProps {
  text: string;
  maxLines?: number;
  charsPerLine?: number;
  size?: string;
  c?: string;
  isHtml?: boolean;
}

const LINE_HEIGHT = 1.6;
const BASE_FONT_PX = 16;

export function ExpandableText({
  text,
  maxLines = 3,
  charsPerLine = 70,
  size,
  c,
  isHtml = false,
}: ExpandableTextProps) {
  const [expanded, setExpanded] = useState(false);
  const [isOverflowing, setIsOverflowing] = useState(false);
  const [fullHeight, setFullHeight] = useState(0);
  const htmlRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!isHtml || !htmlRef.current) return;
    const el = htmlRef.current;
    const scroll = el.scrollHeight;
    setFullHeight(scroll);
    setIsOverflowing(scroll > el.clientHeight + 1);
  }, [isHtml, text]);

  const toggle = () => setExpanded((v) => !v);

  if (isHtml) {
    const maxHeightPx = maxLines * BASE_FONT_PX * LINE_HEIGHT;
    return (
      <Stack gap={4}>
        <Box
          ref={htmlRef}
          c={c}
          dangerouslySetInnerHTML={{ __html: text }}
          onClick={isOverflowing && !expanded ? toggle : undefined}
          style={{
            lineHeight: LINE_HEIGHT,
            maxHeight: expanded ? `${fullHeight}px` : `${maxHeightPx}px`,
            overflow: "hidden",
            transition: "max-height 300ms ease",
            cursor: isOverflowing && !expanded ? "pointer" : undefined,
          }}
        />
        {isOverflowing && (
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
