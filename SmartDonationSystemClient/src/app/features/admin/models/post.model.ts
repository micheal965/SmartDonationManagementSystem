export interface PostToReturnDto {
  id: string;
  title: string;
  content: string;
  status: string;
  createdAt: Date;
  postPicture?: string;
  postAttachments?: string[];

  categoryName: string;

  requesterName?: string;
  requesterPicture?: string;
}
