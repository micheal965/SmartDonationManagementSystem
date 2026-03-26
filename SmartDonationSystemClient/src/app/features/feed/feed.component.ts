import { UserService } from './../../core/services/user.service';
import { Component, inject, OnInit } from '@angular/core';
import { CardComponent } from '../card/card.component';
import { FeedService } from './services/feed.service';
import { Post } from './models/post.model';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { InfiniteScrollDirective } from '../../shared/directives/infinite-scroll.directive';
import { CreatePostComponent } from '../create-post/create-post.component';
import { ToastrService } from 'ngx-toastr';
import { MatIcon } from '@angular/material/icon';
import { NgxSpinnerModule } from 'ngx-spinner';
import { AuthService } from '../auth/services/auth.service';
import { CdkAriaLive } from '../../../../node_modules/@angular/cdk/a11y/index';
@Component({
  selector: 'app-feed',
  standalone: true,
  imports: [
    CardComponent,
    NgIf,
    NgFor,
    InfiniteScrollDirective,
    NgClass,
    CreatePostComponent,
  ],
  templateUrl: './feed.component.html',
  styleUrl: './feed.component.scss',
})
export class FeedComponent implements OnInit {
  UserService = inject(UserService);
  authService = inject(AuthService);
  private feedService = inject(FeedService);
  private toastr = inject(ToastrService);
  posts: Post[] = [];
  pageNumber = 1;
  pageSize = 4;

  loading = false;
  hasNext = true;
  isOpen = false;
  isModalOpen = false;

  filter: 'All' | 'Medical' | 'Jobs' = 'All';
  filters: ('All' | 'Medical' | 'Jobs')[] = ['All', 'Medical', 'Jobs'];
  sort: 'Recent' | 'Urgent' | 'MostViewed' = 'Urgent';
  sortOptions = [
    { label: 'Recent', value: 'Recent' },
    { label: 'Urgent', value: 'Urgent' },
    { label: 'Most Viewed', value: 'MostViewed' },
  ];

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
  onCreatePost(formData: any) {
    this.feedService
      .createPost(
        formData.title,
        formData.content,
        formData.categoryId,
        formData.postPicture,
        formData.attachments,
      )
      .subscribe({
        next: () => {
          this.toastr.success('Post created successfully!');
          this.closeModal();
        },
      });
  }

  trackById(index: number, item: Post) {
    return item.id;
  }

  onFilterClick(filter: 'All' | 'Medical' | 'Jobs') {
    this.filter = filter;
    this.reset();
    this.loadPosts();
  }

  onSortClick(sort: 'Recent' | 'Urgent' | 'MostViewed') {
    this.sort = sort;
    this.reset();
    this.loadPosts();
  }
  private reset() {
    this.pageNumber = 1;
    this.hasNext = true;
    this.posts = [];
  }
  openModal() {
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  get selectedLabel() {
    return this.sortOptions.find((x) => x.value === this.sort)?.label;
  }

  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }

  selectSort(item: any) {
    this.sort = item.value;
    this.isOpen = false;
    this.onSortClick(item.value);
  }
}
