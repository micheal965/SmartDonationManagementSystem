import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { FeedService } from '../feed/services/feed.service';

export const postCommentsResolver: ResolveFn<any> = async (route) => {
  const feedService = inject(FeedService);
  const postId = route.paramMap.get('id')!;
  const comments = await firstValueFrom(
    feedService.getPostCommentsById(Number(postId)),
  );
  return comments;
};
