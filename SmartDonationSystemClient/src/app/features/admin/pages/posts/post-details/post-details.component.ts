import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { ToastrService } from 'ngx-toastr';
import { PostToReturnDto } from '../../../models/post.model';
import { PostsService } from '../../../services/posts.service';
import { finalize } from 'rxjs';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-post-details',
  standalone: true,
  imports: [CommonModule, MatIconModule, RouterModule],
  providers: [DatePipe],
  templateUrl: './post-details.component.html',
  styleUrl: './post-details.component.scss',
})
export class PostDetailsComponent implements OnInit {
  post!: PostToReturnDto;
  isLoading = false;

  private route = inject(ActivatedRoute);
  private postsService = inject(PostsService);
  private toastr = inject(ToastrService);
  private titleService = inject(Title);

  ngOnInit(): void {
    this.route.data.subscribe(({ post }) => {
      this.post = post;
      this.titleService.setTitle(`Post | ${this.post.title}`);
    });
  }

  updateStatus(status: 'Approved' | 'Rejected') {
    this.isLoading = true;
    this.postsService
      .updatePostStatus(this.post.id, status)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (res) => {
          this.toastr.success(res.message);
          this.post.status = status;
        },
      });
  }

  getStatusClass(status: string): string {
    const s = status.toLowerCase();
    switch (s) {
      case 'pending':
        return 'bg-yellow-100 text-yellow-700 border-yellow-200';
      case 'approved':
        return 'bg-green-100 text-green-700 border-green-200';
      case 'rejected':
        return 'bg-red-100 text-red-700 border-red-200';
      case 'completed':
        return 'bg-blue-100 text-blue-700 border-blue-200';
      default:
        return 'bg-gray-100 text-gray-700 border-gray-200';
    }
  }
  getFileIcon(url: string): string {
    if (!url) return 'attach_file';

    const lower = url.toLowerCase();

    if (lower.endsWith('.pdf')) return 'picture_as_pdf';
    if (lower.match(/\.(jpg|jpeg|png|gif|webp)$/)) return 'image';
    if (lower.match(/\.(doc|docx)$/)) return 'description';
    if (lower.match(/\.(xls|xlsx)$/)) return 'table_chart';
    if (lower.match(/\.(zip|rar)$/)) return 'folder_zip';

    return 'link';
  }
}
