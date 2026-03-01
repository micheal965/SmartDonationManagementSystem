import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { FeedService } from '../feed/services/feed.service';
import { firstValueFrom } from 'rxjs';

export const postDetailsResolver: ResolveFn<any> = async (route) => {
  const feedService = inject(FeedService);
  const postId = route.paramMap.get('id')!;
  const post = await firstValueFrom(feedService.getPostById(Number(postId)));
  return post;
};
