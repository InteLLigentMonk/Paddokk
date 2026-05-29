import { Button } from "@mantine/core";
import { UserMinus, UserPlus } from "lucide-react";
import { useCurrentUser } from "@/hooks/use-current-user";
import { useToggleFollow } from "@/lib/api/users.queries";

interface FollowButtonProps {
  userId: string;
  username: string;
  isFollowedByMe: boolean;
}

export function FollowButton({
  userId,
  username,
  isFollowedByMe,
}: FollowButtonProps) {
  const { data: currentUser } = useCurrentUser();
  const toggleFollow = useToggleFollow(userId, username);

  // You can't follow yourself, so hide the button on your own profile.
  if (currentUser?.id === userId) {
    return null;
  }

  return (
    <Button
      variant={isFollowedByMe ? "default" : "filled"}
      leftSection={
        isFollowedByMe ? <UserMinus size={16} /> : <UserPlus size={16} />
      }
      loading={toggleFollow.isPending}
      onClick={() => toggleFollow.mutate(isFollowedByMe)}
      aria-label={
        isFollowedByMe ? `Unfollow ${username}` : `Follow ${username}`
      }
    >
      {isFollowedByMe ? "Unfollow" : "Follow"}
    </Button>
  );
}
