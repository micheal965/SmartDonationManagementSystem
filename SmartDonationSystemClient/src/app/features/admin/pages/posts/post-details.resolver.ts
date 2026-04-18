import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { PostsService } from '../../services/posts.service';

export const postDetailsResolver: ResolveFn<any> = async (route) => {
  const postsService = inject(PostsService);
  const postId = route.paramMap.get('id')!;
  const post = await firstValueFrom(postsService.getPostDetails(postId));
  return post;
};
