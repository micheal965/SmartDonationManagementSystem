import { Component } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { PaginatedResponse } from '../../../../shared/models/paginated-response.model';
import { PostToReturnDto } from '../../models/post.model';
import { PostsService } from '../../services/posts.service';
import { NgFor, NgClass, DatePipe } from '@angular/common';

@Component({
  selector: 'app-posts',
  standalone: true,
  imports: [MatIcon, NgFor, NgClass, DatePipe],
  templateUrl: './posts.component.html',
  styleUrl: './posts.component.scss',
})
export class PostsComponent {
  PaginatedPosts: PaginatedResponse<PostToReturnDto> | null = null;
  selectedFilter: string | null = null;
  pageNumber = 1;
  pageSize = 8;

  filters = [
    { label: 'All Posts', value: null },
    { label: 'Pending', value: 'Pending' },
    { label: 'Rejected', value: 'Rejected' },
    { label: 'Published', value: 'Approved' },
    { label: 'Completed', value: 'Completed' },
  ];
  statusStyleMap: Record<string, string> = {
    pending:
      'bg-yellow-500/10 text-yellow-600 border border-yellow-500/20 rounded-full shadow-sm backdrop-blur-md',

    rejected: 'bg-red-500 text-white rounded-full shadow-md',

    approved:
      'bg-emerald-500/10 text-emerald-600 border border-emerald-500/20 rounded-full',

    completed:
      'bg-blue-500/10 text-blue-600 border border-blue-500/20 rounded-full',

    published:
      'bg-green-500/10 text-green-600 border border-green-500/20 rounded-full shadow-sm',

    default:
      'bg-slate-500/10 text-slate-600 border border-slate-300 rounded-full',
  };
  constructor(private postService: PostsService) {}

  ngOnInit(): void {
    this.loadPosts();
  }
  loadPosts(): void {
    this.postService
      .getUsers(this.pageNumber, this.pageSize, this.selectedFilter)
      .subscribe({
        next: (res) => {
          this.PaginatedPosts = {
            ...res,
            items: [...(this.PaginatedPosts?.items ?? []), ...res.items],
          };
          console.log(res);
        },
      });
  }
  onFilter(filter: string | null) {
    this.pageNumber = 1;
    this.selectedFilter = filter;

    this.PaginatedPosts = null;
    this.loadPosts();
  }
  onLoadMore() {
    this.pageNumber++;
    this.loadPosts();
  }
  getStatusClass(status: string | null): string {
    if (!status) return this.statusStyleMap['default'];
    const normalized = status.toLowerCase();
    return this.statusStyleMap[normalized] || this.statusStyleMap['default'];
  }
}
