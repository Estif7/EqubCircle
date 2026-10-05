export interface CircleMember {
  membershipId: string;
  userId: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  roleInCircle: 'ORGANIZER' | 'MEMBER' | string;
  payoutOrder: number | null;
  status: 'ACTIVE' | 'INACTIVE' | string;
  joinedAt: string;
}

export interface CircleSummary {
  id: string;
  name: string;
  contributionAmount: number;
  frequency: 'DAILY' | 'WEEKLY' | 'MONTHLY' | string;
  memberLimit: number;
  memberCount: number;
  status: 'OPEN' | 'ACTIVE' | 'COMPLETED' | 'CANCELLED' | string;
  organizerId: string;
  organizerName: string;
  isOrganizer: boolean;
  isMember: boolean;
  createdAt: string;
}

export interface CircleDetails {
  id: string;
  name: string;
  contributionAmount: number;
  frequency: 'DAILY' | 'WEEKLY' | 'MONTHLY' | string;
  memberLimit: number;
  memberCount: number;
  status: 'OPEN' | 'ACTIVE' | 'COMPLETED' | 'CANCELLED' | string;
  organizerId: string;
  organizerName: string;
  isOrganizer: boolean;
  isMember: boolean;
  createdAt: string;
  startedAt: string | null;
  completedAt: string | null;
  members: CircleMember[];
}

export interface CreateCircleRequest {
  name: string;
  contributionAmount: number;
  frequency: 'DAILY' | 'WEEKLY' | 'MONTHLY' | string;
  memberLimit: number;
}
