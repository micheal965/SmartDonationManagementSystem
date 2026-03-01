import { UserReactionsDto } from './models/user-reactions.model';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserProfile } from '../../shared/models/user-profile.model';
import { Title } from '@angular/platform-browser';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { UserPostsDto } from './models/user-posts.model';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../auth/services/auth.service';
import { UserService } from '../../core/services/user.service';
import { ProfilePostComponent } from './profile-post/profile-post.component';
import { ProfileLikeComponent } from './profile-like/profile-like.component';
import { EditProfileComponent } from './edit-profile/edit-profile.component';
import { EditUserModel } from './models/edit-user-profile.model';
import { ProfilePictureModalComponent } from './profile-picture-modal/profile-picture-modal.component';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    NgClass,
    NgIf,
    NgFor,
    MatIconModule,
    ProfilePostComponent,
    ProfileLikeComponent,
    EditProfileComponent,
    ProfilePictureModalComponent,
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private titleService = inject(Title);
  private toastr = inject(ToastrService);
  private authService = inject(AuthService);
  private userService = inject(UserService);

  userReactionsDto: UserReactionsDto | null = null;
  userPostsDto: UserPostsDto | null = null;
  user!: UserProfile;
  currentUser!: UserProfile;
  activeTab: 'posts' | 'likes' | 'comments' = 'posts';
  isCurrentUserProfile: boolean = false;
  showEditModal: boolean = false;
  showProfilePictureModal: boolean = false;

  ngOnInit(): void {
    this.route.data.subscribe(({ user }) => {
      this.user = user;
      if (user.id == this.authService.userData.id) {
        this.isCurrentUserProfile = true;
        this.currentUser = this.user;
      }
      this.userService.getUserReacts(user.id).subscribe({
        next: (reactionsDto) => (this.userReactionsDto = reactionsDto),
      });
      this.userService.getUserPosts(user.id).subscribe({
        next: (postsDto) => (this.userPostsDto = postsDto),
      });
      this.titleService.setTitle(`${this.user.fullName.split(' ')[0]} Profile`);
    });
  }

  updateUser(updatedUser: EditUserModel) {
    this.user = { ...this.user, ...updatedUser };
    this.userService.updateUser(updatedUser).subscribe({
      next: (res) => this.toastr.success(res.message),
    });
    this.showEditModal = false;
  }

  deleteUser() {
    this.userService.deleteUserSoft().subscribe({
      next: () => this.toastr.success('Account Deleted Successfully'),
    });
  }

  shareProfile() {
    const profileUrl = window.location.href;
    if (navigator.share) {
      navigator
        .share({
          title: 'Check out this profile',
          text: 'Take a look at this profile!',
          url: profileUrl,
        })
        .catch((err) => {
          this.toastr.error('Share cancelled or failed', err);
        });
    } else {
      this.copyToClipboard(profileUrl);
    }
  }
  setTab(tab: 'posts' | 'likes' | 'comments') {
    this.activeTab = tab;
  }
  onUpdatePhoto(profilePicture: File) {
    this.userService.updateProfilePicture(profilePicture).subscribe({
      next: (res) => {
        this.toastr.success('Profile Picture Updated Successfully');
        this.currentUser.pictureUrl = res.pictureUrl;

        this.userService.profile.update((profile) => {
          if (!profile) return null;
          return { ...profile, pictureUrl: res.pictureUrl };
        });
      },
    });
  }

  onDeletePhoto() {
    this.userService.deleteUserProfilePicture().subscribe({
      next: () => {
        this.toastr.success('Profile Picture Deleted Successfully');

        this.currentUser.pictureUrl = '';

        this.userService.profile.update((profile) => {
          if (!profile) return null;
          return { ...profile, pictureUrl: '' };
        });

        this.showProfilePictureModal = false;
      },
    });
  }
  private copyToClipboard(text: string) {
    navigator.clipboard.writeText(text).then(() => {
      this.toastr.success('Profile link copied!');
    });
  }
}
