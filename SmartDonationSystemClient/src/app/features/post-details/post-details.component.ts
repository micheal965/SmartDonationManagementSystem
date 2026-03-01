import { PriorityClassPipe } from './../../shared/pipes/priority-class.pipe';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PostDetails } from './models/post-details.model';
import { Title } from '@angular/platform-browser';
import { MatIconModule } from '@angular/material/icon';
import { TimeAgoPipe } from '../../shared/pipes/time-ago.pipe';
import { NgIf, NgClass } from '@angular/common';
import { UserService } from '../../core/services/user.service';
import { PriorityLabelPipe } from '../../shared/pipes/priority-label.pipe';

@Component({
  selector: 'app-post-details',
  standalone: true,
  imports: [
    MatIconModule,
    TimeAgoPipe,
    NgIf,
    PriorityLabelPipe,
    PriorityClassPipe,
    NgClass
],
  templateUrl: './post-details.component.html',
  styleUrl: './post-details.component.scss',
})
export class PostDetailsComponent implements OnInit {
  post!: PostDetails;
  userService = inject(UserService);
  private route = inject(ActivatedRoute);
  private titleService = inject(Title);

  ngOnInit() {
    this.post = this.route.snapshot.data['post'];
    if (this.post) this.titleService.setTitle(this.post.title);
  }
}
