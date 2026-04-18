import { Component, inject, OnInit, PLATFORM_ID } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { CategoryService } from '../../core/services/category.service';
import { Category } from '../../shared/models/category-model';
import { NgFor, NgIf, isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [MatIcon, RouterLink, NgFor, NgIf],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss',
})
export class CategoriesComponent implements OnInit {
  private categoryService = inject(CategoryService);
  categories: Category[] = [];

  ngOnInit(): void {
    this.categoryService.getCategories().subscribe({
      next: (cats) => (this.categories = cats),
    });
  }

  getCategoryIcon(name: string): string {
    const icons: { [key: string]: string } = {
      'food': 'restaurant',
      'health': 'medical_services',
      'medical': 'medical_services',
      'education': 'school',
      'clothing': 'checkroom',
      'shelter': 'home',
      'money': 'payments',
      'water': 'water_drop',
      'environment': 'eco',
      'jobs': 'work',
      'special cases': 'star'
    };

    return icons[name.toLowerCase()] || 'category';
  }
}
