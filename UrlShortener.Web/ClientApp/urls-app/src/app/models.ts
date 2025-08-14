export interface UrlListItemDto {
  id: number;
  originalUrl: string;
  shortCode: string;
  createdById: number;
  clicks: number;
  canDelete: boolean;
}
export interface UrlsPageDto {
  isAuthorized: boolean;
  isAdmin: boolean;
  currentUserId: number | null;
  items: UrlListItemDto[];
}
