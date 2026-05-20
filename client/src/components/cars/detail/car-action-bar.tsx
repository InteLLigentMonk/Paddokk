import { useState } from "react";
import { Group, Button, Text } from "@mantine/core";
import { Heart, Bell, Share2 } from "lucide-react";
import { useQueryClient } from "@tanstack/react-query";
import { useNotifications } from "@/integrations/mantine";
import type { UserCarDto } from "@/generated/api/schemas";
import {
  likeUserCarFn,
  unlikeUserCarFn,
  subscribeToUserCarFn,
  unsubscribeFromUserCarFn,
} from "@/lib/api/user-cars.server";

interface CarActionBarProps {
  car: UserCarDto;
}

export function CarActionBar({ car }: CarActionBarProps) {
  const queryClient = useQueryClient();
  const notifications = useNotifications();
  const [likeLoading, setLikeLoading] = useState(false);
  const [subscribeLoading, setSubscribeLoading] = useState(false);

  const handleLike = async () => {
    setLikeLoading(true);
    try {
      if (car.isLiked) {
        await unlikeUserCarFn({ data: { carId: Number(car.id) } });
      } else {
        await likeUserCarFn({ data: { carId: Number(car.id) } });
      }
      queryClient.invalidateQueries({ queryKey: ["user-car-by-slug"] });
    } catch {
      notifications.error({ message: "Failed to update like" });
    } finally {
      setLikeLoading(false);
    }
  };

  const handleSubscribe = async () => {
    setSubscribeLoading(true);
    try {
      if (car.isSubscribed) {
        await unsubscribeFromUserCarFn({ data: { carId: Number(car.id) } });
      } else {
        await subscribeToUserCarFn({ data: { carId: Number(car.id) } });
      }
      queryClient.invalidateQueries({ queryKey: ["user-car-by-slug"] });
    } catch {
      notifications.error({ message: "Failed to update follow" });
    } finally {
      setSubscribeLoading(false);
    }
  };

  const handleShare = () => {
    if (navigator.share) {
      navigator.share({ title: document.title, url: window.location.href });
    } else {
      navigator.clipboard.writeText(window.location.href);
      notifications.success({ message: "Link copied to clipboard" });
    }
  };

  return (
    <Group gap="xs" justify="space-between">
      <Group gap="xs">
        {!car.isOwner && (
          <>
            <Button
              variant="subtle"
              color="light-dark(var(--mantine-color-dark-7), var(--mantine-color-gray-3))"
              bd="1px solid light-dark(var(--mantine-color-gray-4), var(--mantine-color-dark-5))"
              size="sm"
              leftSection={
                <Heart
                  size={14}
                  fill={car.isLiked ? "var(--mantine-color-red-5)" : "none"}
                  stroke={
                    car.isLiked ? "var(--mantine-color-red-5)" : "currentColor"
                  }
                />
              }
              onClick={handleLike}
              loading={likeLoading}
            >
              <Text fz={12}>{Number(car.likeCount)}</Text>
            </Button>

            <Button
              variant="subtle"
              color="light-dark(var(--mantine-color-dark-7), var(--mantine-color-gray-3))"
              bd="1px solid light-dark(var(--mantine-color-gray-4), var(--mantine-color-dark-5))"
              size="sm"
              leftSection={
                <Bell
                  size={14}
                  fill={
                    car.isSubscribed ? "var(--mantine-color-blue-5)" : "none"
                  }
                  stroke={
                    car.isSubscribed
                      ? "var(--mantine-color-blue-5)"
                      : "currentColor"
                  }
                />
              }
              onClick={handleSubscribe}
              loading={subscribeLoading}
            >
              {car.isSubscribed ? "Following" : "Follow"}
            </Button>
          </>
        )}
      </Group>

      <Button
        variant="subtle"
        size="sm"
        leftSection={<Share2 size={14} />}
        onClick={handleShare}
      >
        Share
      </Button>
    </Group>
  );
}
