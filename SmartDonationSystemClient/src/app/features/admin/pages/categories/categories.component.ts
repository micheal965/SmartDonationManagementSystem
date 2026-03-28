import { NgIf, NgForOf, NgClass } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { CreateCategoryModalComponent } from './create-category-modal/create-category-modal.component';
import { CategoriesService } from '../../services/categories.service';
import { ToastrService } from 'ngx-toastr';
import { CategoryToReturnDto } from '../../models/category.model';
import { UpdateCategoryDto } from '../../models/update-category.model';
import { UpdateCategoryModalComponent } from './update-category-modal/update-category-modal.component';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [
    MatIcon,
    NgIf,
    CreateCategoryModalComponent,
    NgForOf,
    NgClass,
    UpdateCategoryModalComponent,
  ],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss',
})
export class CategoriesComponent implements OnInit {
  private categoriesService = inject(CategoriesService);
  private toastr = inject(ToastrService);
  categories: CategoryToReturnDto[] = [];
  filteredCategories: CategoryToReturnDto[] = [];
  selectedFilter: 'all' | 'popular' = 'all';
  isCreateOpen = false;
  isEditOpen = false;
  isMenuOpen = false;
  iconMap: { [key: string]: string } = {
    jobs: 'work',
    medical: 'local_hospital',
    'special-cases': 'report_problem',
    default: 'category',
  };
  selectedItemToUpdate = {
    id: '',
    name: '',
    description: '',
  };
  ngOnInit(): void {
    this.categoriesService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.filteredCategories = categories;
      },
    });
  }
  getIcon(category: string): string {
    if (!category) return this.iconMap['default'];

    const key = category.toLowerCase().trim();
    return this.iconMap[key] || this.iconMap['default'];
  }
  openModal() {
    this.isCreateOpen = true;
  }
  closeModal() {
    this.isCreateOpen = false;
  }
  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }
  onEdit(selectedCategory: any) {
    this.isMenuOpen = false;
    this.isEditOpen = true;
    this.selectedItemToUpdate = selectedCategory;
  }
  showAll() {
    this.selectedFilter = 'all';
    this.filteredCategories = this.categories;
  }
  showPopular() {
    this.selectedFilter = 'popular';

    this.filteredCategories = [...this.categories]
      .sort((a, b) => b.totalPosts - a.totalPosts)
      .filter((c) => c.totalPosts > 0);
  }

  onAddCategory(data: { categoryName: string; description: string }) {
    this.categoriesService
      .createCategory(data.categoryName, data.description)
      .subscribe({
        next: (res) => {
          this.toastr.success(res.message);
          this.categories.push(res.data);
        },
      });
  }
  onUpdate(updatedData: any) {
    const updateCategoryDto: UpdateCategoryDto = {
      oldCategoryId: updatedData.id,
      newCategoryName: updatedData.name,
      newDescription: updatedData.description,
    };
    this.categoriesService.updateCategory(updateCategoryDto).subscribe({
      next: (res) => this.toastr.success(res.message),
    });
  }
  deleteCategory(id: number) {
    this.categoriesService.deleteCategory(id).subscribe({
      next: (res) => {
        this.toastr.success(res.message);
        this.categories = this.categories.filter((c) => c.id != id);
        this.filteredCategories = this.categories;
      },
    });
  }
}
