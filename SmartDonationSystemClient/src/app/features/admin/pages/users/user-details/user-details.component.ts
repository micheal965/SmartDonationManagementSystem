import { Component, inject, OnInit } from '@angular/core';
import { UserProfile } from '../../../../../shared/models/user-profile.model';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { CommonModule, DatePipe } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { UserToReturnDto } from '../../../models/user-model';
import { UsersService } from '../../../services/users.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [CommonModule, MatIconModule, RouterModule],
  providers: [DatePipe],
  templateUrl: './user-details.component.html',
  styleUrl: './user-details.component.scss',
})
export class UserDetailsComponent implements OnInit {
  user!: UserToReturnDto;
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);
  private titleService = inject(Title);
  private usersService = inject(UsersService);

  ngOnInit(): void {
    this.route.data.subscribe(({ user }) => {
      this.user = user;
      this.titleService.setTitle(`${this.user.fullName.split(' ')[0]} Profile`);
    });
  }
  toggleUserStatus(user: UserToReturnDto) {
    this.usersService.DeleteUserSoft(user.id).subscribe({
      next: (res) => {
        user.isSoftDeleted = !user.isSoftDeleted;
        this.toastr.success(res.message);
      },
    });
  }
}
