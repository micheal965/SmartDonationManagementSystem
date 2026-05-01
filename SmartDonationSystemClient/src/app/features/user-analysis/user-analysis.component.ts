import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';
import { ShortNumberPipe } from '../../shared/pipes/short-number.pipe';
import { UserAnalysisService, UserAnalysis } from '../../core/services/user-analysis.service';

@Component({
  selector: 'app-user-analysis',
  standalone: true,
  imports: [CommonModule, MatIconModule, BaseChartDirective, ShortNumberPipe],
  templateUrl: './user-analysis.component.html'
})
export class UserAnalysisComponent implements OnInit {
  private userAnalysisService = inject(UserAnalysisService);
  analysisData?: UserAnalysis;

  public donorTrendChartData: ChartConfiguration['data'] = {
    datasets: [{ 
      data: [], 
      label: 'Donated (EGP)', 
      borderColor: '#10b981', 
      backgroundColor: 'rgba(16, 185, 129, 0.2)', 
      fill: 'origin', 
      tension: 0.4,
      pointBackgroundColor: '#ffffff',
      pointBorderColor: '#10b981',
      pointBorderWidth: 2,
      pointRadius: 4,
      pointHoverRadius: 6,
      borderWidth: 3
    }],
    labels: [],
  };

  public requesterTrendChartData: ChartConfiguration['data'] = {
    datasets: [{ 
      data: [], 
      label: 'Raised (EGP)', 
      borderColor: '#8b5cf6', 
      backgroundColor: 'rgba(139, 92, 246, 0.2)', 
      fill: 'origin', 
      tension: 0.4,
      pointBackgroundColor: '#ffffff',
      pointBorderColor: '#8b5cf6',
      pointBorderWidth: 2,
      pointRadius: 4,
      pointHoverRadius: 6,
      borderWidth: 3
    }],
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
        backgroundColor: 'rgba(15, 23, 42, 0.9)',
        titleFont: { size: 13, family: "'Inter', sans-serif" },
        bodyFont: { size: 13, family: "'Inter', sans-serif" },
        padding: 12,
        cornerRadius: 8,
        displayColors: false
      } 
    },
    scales: { 
      x: { 
        grid: { display: false },
        ticks: { color: '#64748b', font: { family: "'Inter', sans-serif" } }
      }, 
      y: { 
        beginAtZero: true, 
        grid: { color: '#f1f5f9', drawTicks: false },
        border: { dash: [5, 5], display: false },
        ticks: { color: '#64748b', padding: 10, font: { family: "'Inter', sans-serif" } }
      } 
    },
    interaction: {
      mode: 'nearest',
      axis: 'x',
      intersect: false
    }
  };

  ngOnInit(): void {
    this.userAnalysisService.getMyImpact().subscribe(data => {
      this.analysisData = data;
      this.updateCharts();
    });
  }

  private updateCharts() {
    if (!this.analysisData) return;

    this.donorTrendChartData = {
      ...this.donorTrendChartData,
      labels: this.analysisData.donorImpact.donationTrend.map(t => new Date(t.date).toLocaleDateString()),
      datasets: [{ ...this.donorTrendChartData.datasets[0], data: this.analysisData.donorImpact.donationTrend.map(t => t.value) }]
    };

    this.requesterTrendChartData = {
      ...this.requesterTrendChartData,
      labels: this.analysisData.requesterImpact.fundsRaisedTrend.map(t => new Date(t.date).toLocaleDateString()),
      datasets: [{ ...this.requesterTrendChartData.datasets[0], data: this.analysisData.requesterImpact.fundsRaisedTrend.map(t => t.value) }]
    };
  }
}
