import {
  Component,
  inject,
  OnInit,
  ViewChildren,
  QueryList,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { AnalysisModel } from '../../models/analysis.model';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartData, ChartType } from 'chart.js';
import { ShortNumberPipe } from '../../../../shared/pipes/short-number.pipe';
import { FormsModule } from '@angular/forms';
import { AnalysisService } from '../../services/analysis.service';

@Component({
  selector: 'app-analysis',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    BaseChartDirective,
    ShortNumberPipe,
    FormsModule,
  ],
  templateUrl: './analysis.component.html',
})
export class AnalysisComponent implements OnInit {
  @ViewChildren(BaseChartDirective) charts?: QueryList<BaseChartDirective>;
  private analysisService = inject(AnalysisService);
  analysisData?: AnalysisModel;

  fromDate: string;
  toDate: string;

  constructor() {
    const today = new Date();
    const thirtyDaysAgo = new Date();
    thirtyDaysAgo.setDate(today.getDate() - 30);

    this.toDate = today.toISOString().split('T')[0];
    this.fromDate = thirtyDaysAgo.toISOString().split('T')[0];
  }

  // Trend Chart
  public lineChartData: ChartConfiguration['data'] = {
    datasets: [
      {
        data: [],
        label: 'Donation Amount (EGP)',
        borderColor: '#4f46e5',
        backgroundColor: 'rgba(79, 70, 229, 0.1)',
        fill: 'origin',
        tension: 0.4,
      },
    ],
    labels: [],
  };

  public lineChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: {
        mode: 'index',
        intersect: false,
      },
    },
    scales: {
      y: { beginAtZero: true },
    },
  };

  // Category Trend Chart
  public categoryTrendChartData: ChartConfiguration['data'] = {
    labels: [],
    datasets: [],
  };

  public categoryTrendChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'bottom',
        labels: {
          usePointStyle: true,
          padding: 20,
          font: { size: 12, family: 'Inter, sans-serif' },
        },
      },
      tooltip: {
        mode: 'index',
        intersect: false,
      },
    },
    scales: {
      x: {
        grid: { display: false },
        ticks: { font: { weight: 'bold' } },
      },
      y: {
        grid: { color: '#f1f5f9' },
        beginAtZero: true,
      },
    },
  };

  // Status Chart
  public doughnutChartLabels: string[] = [];
  public doughnutChartData: ChartData<'doughnut'> = {
    labels: this.doughnutChartLabels,
    datasets: [
      {
        data: [],
        backgroundColor: [
          '#16a34a', // green-600
          '#f59e0b',
          '#4f46e5', // indigo-600
          '#64748b', // slate-500 → neutral fallback
        ],
        hoverOffset: 15,
        borderWidth: 0,
      },
    ],
  };
  public doughnutChartType: ChartType = 'doughnut';
  public doughnutChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'bottom',
        labels: {
          usePointStyle: true,
          padding: 25,
          font: { size: 12, family: 'Inter, sans-serif' },
        },
      },
    },
  };

  ngOnInit(): void {
    this.fetchData();
  }

  fetchData(): void {
    this.analysisService
      .getAnalysis(this.fromDate, this.toDate)
      .subscribe((data) => {
        this.analysisData = data;
        this.updateCharts();
      });
  }

  private updateCharts(): void {
    if (!this.analysisData) return;

    // Update Trend - Create new object reference to trigger change detection
    this.lineChartData = {
      ...this.lineChartData,
      labels: this.analysisData.donationTrend.map((t) =>
        new Date(t.date).toLocaleDateString(),
      ),
      datasets: [
        {
          ...this.lineChartData.datasets[0],
          data: this.analysisData.donationTrend.map((t) => t.value),
        },
      ],
    };

    // Update Categories
    const colors = [
      '#6366f1',
      '#ec4899',
      '#10b981',
      '#f59e0b',
      '#3b82f6',
      '#8b5cf6',
      '#ef4444',
      '#14b8a6',
    ];
    this.categoryTrendChartData = {
      ...this.categoryTrendChartData,
      labels:
        this.analysisData.categoryTrends[0]?.trends.map((t) =>
          new Date(t.date).toLocaleDateString(),
        ) || [],
      datasets: this.analysisData.categoryTrends.map((c, index) => ({
        data: c.trends.map((t) => t.value),
        label: c.categoryName,
        borderColor: colors[index % colors.length],
        backgroundColor: colors[index % colors.length] + '1A', // Changed to lighter opacity for overlapping shadows
        fill: 'origin',
        tension: 0.4,
        borderWidth: 2,
        pointRadius: 2,
        pointHoverRadius: 4,
      })),
    };

    // Update Status
    this.doughnutChartLabels = this.analysisData.statusBreakdown.map(
      (s) => s.status,
    );
    this.doughnutChartData = {
      ...this.doughnutChartData,
      labels: this.doughnutChartLabels,
      datasets: [
        {
          ...this.doughnutChartData.datasets[0],
          data: this.analysisData.statusBreakdown.map((s) => s.count),
        },
      ],
    };

    // Explicitly update charts if they are already rendered
    setTimeout(() => {
      this.charts?.forEach((chart) => chart.update());
    });
  }
}
