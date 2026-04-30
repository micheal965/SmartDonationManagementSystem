import { RouterModule } from '@angular/router';
import {
  SidebarService,
  SidebarData,
} from '../../../core/services/sidebar.service';
import { TimeAgoPipe } from '../../../shared/pipes/time-ago.pipe';
import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { PriorityLabelPipe } from "../../../shared/pipes/priority-label.pipe";
import { PriorityClassPipe } from "../../../shared/pipes/priority-class.pipe";
@Component({
  selector: 'app-user-right-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, TimeAgoPipe, PriorityLabelPipe, PriorityClassPipe],
  templateUrl: './user-right-sidebar.component.html',
  styleUrl: './user-right-sidebar.component.scss',
})
export class UserRightSidebarComponent implements OnInit {
  private sidebarService = inject(SidebarService);
  sidebarData?: SidebarData;

  ngOnInit(): void {
    this.sidebarService.getSidebarData().subscribe({
      next: (data) => {
        this.sidebarData = data;
      },
    });
  }
}
