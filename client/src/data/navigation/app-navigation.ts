import {
  LayoutDashboard,
  BookOpen,
  Compass,
  Car,
  Users,
  Menu,
  Camera,
  Library,
  FileText,
  Upload,
} from 'lucide-react'
import type { NavConfig } from './types'

export const navigationConfig: NavConfig = {
  primary: [
    {
      id: 'dashboard',
      label: 'Dashboard',
      href: '/dashboard',
      icon: LayoutDashboard,
      group: 'primary',
    },
    {
      id: 'journeys',
      label: 'My Journeys',
      href: '/journeys',
      icon: BookOpen,
      group: 'primary',
    },
    {
      id: 'explore',
      label: 'Explore',
      href: '/explore',
      icon: Compass,
      group: 'primary',
    },
    {
      id: 'cars',
      label: 'My Cars',
      href: '/cars',
      icon: Car,
      group: 'primary',
    },
    {
      id: 'community',
      label: 'Community',
      href: '/community',
      icon: Users,
      group: 'primary',
    },
    {
      id: 'more',
      label: 'More',
      icon: Menu,
      group: 'primary',
    },
  ],
  secondary: [
    {
      id: 'gallery',
      label: 'Photo Gallery',
      href: '/gallery',
      icon: Camera,
      group: 'secondary',
    },
    {
      id: 'knowledge',
      label: 'Knowledge Base',
      href: '/knowledge',
      icon: Library,
      group: 'secondary',
    },
  ],
  fabActions: [
    {
      id: 'create-journey',
      label: 'Create Journey',
      icon: BookOpen,
      group: 'primary',
    },
    {
      id: 'add-car',
      label: 'Add Car',
      icon: Car,
      group: 'primary',
    },
    {
      id: 'new-post',
      label: 'New Post',
      icon: FileText,
      group: 'primary',
    },
    {
      id: 'upload-photos',
      label: 'Upload Photos',
      icon: Upload,
      group: 'primary',
    },
  ],
}
