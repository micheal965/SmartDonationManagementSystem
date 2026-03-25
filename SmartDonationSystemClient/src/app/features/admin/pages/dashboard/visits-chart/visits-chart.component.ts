import { NgFor } from '@angular/common';
import { AfterViewInit, Component, Input, ViewChild } from '@angular/core';
import { ChartConfiguration, ChartOptions } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { AnalyticsDto } from '../../../models/dashboard.model';
import { ShortNumberPipe } from '../../../../../shared/pipes/short-number.pipe';

@Component({
  selector: 'app-visits-chart',
  standalone: true,
  imports: [BaseChartDirective, NgFor, ShortNumberPipe],
  templateUrl: './visits-chart.component.html',
  styleUrl: './visits-chart.component.scss',
})
export class VisitsChartComponent implements AfterViewInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;
  @Input() usersCount?: number;

  @Input() set rawData(value: AnalyticsDto[] | undefined) {
    if (value && value.length > 0) {
      this.updateChart(value);
    }
  }
  public footerLabels: string[] = [];
  public lineChartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: {
        mode: 'index',
        intersect: false,
        backgroundColor: '#1e293b',
        titleColor: '#94a3b8',
        bodyColor: '#ffffff',
        bodyFont: { size: 14, weight: 'bold' },
        padding: 12,
        displayColors: false,
        callbacks: {
          label: (context) => ` ${context.parsed.y} unique visits`,
        },
      },
    },
    scales: {
      x: { display: false },
      y: { display: false, grid: { display: false } },
    },
  };
  public lineChartData: ChartConfiguration<'line'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
        borderColor: '#4f46e5',
        borderWidth: 3,
        pointRadius: 0,
        pointHoverRadius: 6,
        pointHoverBackgroundColor: '#4f46e5',
        pointHoverBorderColor: '#fff',
        pointHoverBorderWidth: 3,
        fill: true,
        tension: 0.4,
      },
    ],
  };

  ngAfterViewInit() {
    this.applyGradient();
  }

  // Vertical line plugin
  public tooltipLinePlugin = {
    id: 'tooltipLine',
    afterDraw: (chart: any) => {
      if (chart.tooltip?._active?.length) {
        const ctx = chart.ctx;
        const activePoint = chart.tooltip._active[0];
        const x = activePoint.element.x;
        const lineY = activePoint.element.y;
        const bottomY = chart.chartArea.bottom;

        ctx.save();
        ctx.beginPath();
        ctx.setLineDash([4, 4]);
        ctx.lineWidth = 1.5;
        ctx.strokeStyle = 'rgba(79, 70, 229, 0.6)';
        ctx.moveTo(x, lineY);
        ctx.lineTo(x, bottomY);
        ctx.stroke();
        ctx.restore();
      }
    },
  };
  private updateChart(data: AnalyticsDto[]): void {
    const labels = data.map((item) =>
      new Date(item.date).toLocaleDateString('en-US', {
        month: 'short',
        day: '2-digit',
      }),
    );

    // Calculate footer markers (Start, ~1/3, ~2/3, End)
    this.footerLabels = [
      labels[0],
      labels[Math.floor(labels.length / 3)],
      labels[Math.floor((labels.length / 3) * 2)],
      labels[labels.length - 1],
    ];

    // Update data while preserving style properties
    this.lineChartData.labels = labels;
    this.lineChartData.datasets[0].data = data.map((item) => item.count);

    // Trigger UI updates
    if (this.chart) {
      this.applyGradient();
      this.chart.update();
    }
  }

  private applyGradient() {
    const ctx = this.chart?.chart?.ctx;
    if (ctx) {
      const gradient = ctx.createLinearGradient(0, 0, 0, 300);
      gradient.addColorStop(0, 'rgba(79, 70, 229, 0.25)');
      gradient.addColorStop(1, 'rgba(255, 255, 255, 0)');
      this.lineChartData.datasets[0].backgroundColor = gradient;
    }
  }
}
