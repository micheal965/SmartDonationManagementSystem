import { Component, inject, OnInit } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { UsersService } from '../../services/users.service';
import { UserToReturnDto } from '../../models/user-model';
import { FormsModule } from '@angular/forms';
import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { PaginatedResponse } from '../../../../shared/models/paginated-response.model';
import { ToastrService } from 'ngx-toastr';
import { AddUserModalComponent } from './add-user-modal/add-user-modal.component';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    MatIcon,
    FormsModule,
    NgFor,
    NgIf,
    NgClass,
    DatePipe,
    AddUserModalComponent,
    RouterLink,
  ],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss',
})
export class UsersComponent implements OnInit {
  PaginatedUsers: PaginatedResponse<UserToReturnDto> | null = null;
  private toastr = inject(ToastrService);
  totalCount = 0;

  pageNumber = 1;
  pageSize = 5;

  selectedRole: string | null = null;
  roles = [
    { label: 'All Users', value: null },
    { label: 'Admins', value: 'Admin' },
    { label: 'Donors', value: 'Donor' },
    { label: 'Requesters', value: 'Requester' },
  ];
  isAddUserModelOpen: boolean = false;
  activeDropdownUserId: string | null = null;

  get totalPages() {
    return this.PaginatedUsers
      ? Math.ceil(this.PaginatedUsers.totalCount / this.pageSize)
      : 0;
  }

  get pages(): number[] {
    const total = this.totalPages;
    const current = this.pageNumber;

    if (current <= 2) {
      return [1, 2, 3].filter((p) => p <= total);
    }

    if (current >= total - 1) {
      return [total - 2, total - 1, total].filter((p) => p > 0);
    }

    return [current - 1, current, current + 1];
  }
  constructor(private userService: UsersService) {}

  ngOnInit(): void {
    this.loadUsers();
  }
  loadUsers(): void {
    this.userService
      .getUsers(this.pageNumber, this.pageSize, this.selectedRole)
      .subscribe({
        next: (res) => {
          this.PaginatedUsers = res;
          this.totalCount = res.totalCount;
        },
      });
  }

  filterByRole(role: string | null) {
    this.selectedRole = role;
    this.pageNumber = 1; // reset pagination
    this.loadUsers();
  }

  getFilterClass(role: string | null) {
    return this.selectedRole === role
      ? 'text-indigo-600 border-b-2 border-indigo-600'
      : 'text-slate-400 hover:text-slate-600';
  }

  nextPage(): void {
    this.pageNumber++;
    this.loadUsers();
  }

  prevPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.loadUsers();
    }
  }

  changePageNumber(number: number): void {
    this.pageNumber = number;
    this.loadUsers();
  }
  getTo() {
    return Math.min(
      this.pageNumber * this.pageSize,
      this.PaginatedUsers?.totalCount ?? 0,
    );
  }
  toggleUserSoftDelete(user: UserToReturnDto) {
    this.userService.DeleteUserSoft(user.id).subscribe({
      next: (res) => {
        user.isSoftDeleted = !user.isSoftDeleted;
        this.toastr.success(res.message);
      },
    });
  }
  toggleNewUserModal() {
    this.isAddUserModelOpen = !this.isAddUserModelOpen;
  }
  toggleActions(userId: string | null) {
    this.activeDropdownUserId =
      this.activeDropdownUserId === userId ? null : userId;
  }
}
