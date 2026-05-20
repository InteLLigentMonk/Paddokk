import { useState } from "react";
import { Group, Button, Text } from "@mantine/core";
import { Heart, Bell, Share2 } from "lucide-react";
import { useRouter } from "@tanstack/react-router";
import { useNotifications } from "@/integrations/mantine";
import type { UserCarDto } from "@/generated/api/schemas";
import { likeUserCarFn, unlikeUserCarFn, subscribeToUserCarFn, unsubscribeFromUserCarFn } from "@/lib/api/user-cars.server";

interface CarActionBarProps {
  car: UserCarDto;
}

export function CarActionBar({ car }: CarActionBarProps) {
  const router = useRouter();
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
      await router.invalidate();
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
      await router.invalidate();
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
    <Group gap="xs" wrap="wrap">
      <Button
        variant={car.isLiked ? "filled" : "outline"}
        color="red"
        size="sm"
        leftSection={<Heart size={14} fill={car.isLiked ? "currentColor" : "none"} />}
        onClick={handleLike}
        loading={likeLoading}
      >
        <Text fz={12}>{Number(car.likeCount)}</Text>
      </Button>

      <Button
        variant={car.isSubscribed ? "filled" : "outline"}
        size="sm"
        leftSection={<Bell size={14} fill={car.isSubscribed ? "currentColor" : "none"} />}
        onClick={handleSubscribe}
        loading={subscribeLoading}
      >
        {car.isSubscribed ? "Following" : "Follow"}
      </Button>

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
