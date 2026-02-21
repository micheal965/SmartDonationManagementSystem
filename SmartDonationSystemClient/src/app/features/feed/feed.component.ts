import { Component, inject, OnInit } from '@angular/core';
import { CardComponent } from '../card/card.component';
import { FeedService } from './services/feed.service';
import { Post } from './models/post.model';
import { NgFor } from '@angular/common';
import { InfiniteScrollDirective } from '../../shared/directives/infinite-scroll.directive';
@Component({
  selector: 'app-feed',
  standalone: true,
  imports: [CardComponent, NgFor, InfiniteScrollDirective],
  templateUrl: './feed.component.html',
  styleUrl: './feed.component.scss',
})
export class FeedComponent implements OnInit {
  private feedService = inject(FeedService);
  posts: Post[] = [];
  pageNumber = 1;
  pageSize = 4;
  loading = false;
  hasNext = true;

  ngOnInit(): void {
    this.loadPosts();
  }

  loadPosts(): void {
    if (this.loading || !this.hasNext) return;
    this.loading = true;

    this.feedService
      .getPosts(this.pageNumber, this.pageSize)
      .subscribe(({ items, hasNext }) => {
        this.posts.push(...items);
        this.hasNext = hasNext;
        this.pageNumber++;
        this.loading = false;
      });
  }
  trackById(index: number, item: Post) {
    return item.id;
  }
}
