import { Component, inject, OnInit } from '@angular/core';
import { CardComponent } from '../card/card.component';
import { FeedService } from './services/feed.service';
import { Post } from './models/post.model';
import { NgClass, NgFor } from '@angular/common';
import { InfiniteScrollDirective } from '../../shared/directives/infinite-scroll.directive';
@Component({
  selector: 'app-feed',
  standalone: true,
  imports: [CardComponent, NgFor, InfiniteScrollDirective, NgClass],
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
  filter: 'All' | 'Medical' | 'Jobs' = 'All';
  filters: ('All' | 'Medical' | 'Jobs')[] = ['All', 'Medical', 'Jobs'];
  sort: 'Recent' | 'Urgent' = 'Urgent';

  ngOnInit(): void {
    this.loadPosts();
  }

  loadPosts(): void {
    if (this.loading || !this.hasNext) return;
    this.loading = true;

    const categoryName = this.filter === 'All' ? null : this.filter;
    this.feedService
      .getPosts(this.pageNumber, this.pageSize, categoryName, this.sort)
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

  onFilterClick(filter: 'All' | 'Medical' | 'Jobs') {
    this.filter = filter;
    this.pageNumber = 1;
    this.hasNext = true;
    this.posts = [];
    this.loadPosts();
  }

  onSortClick(sort: 'Recent' | 'Urgent') {
    this.sort = sort;
    this.pageNumber = 1;
    this.hasNext = true;
    this.posts = [];
    this.loadPosts();
  }
  
}
