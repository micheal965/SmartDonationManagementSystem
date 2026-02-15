import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiBaseUrl } from '../../../core/utils/app.config';

@Injectable({ providedIn: 'root' })
export class CloudService {
  constructor(private http: HttpClient) {}

  getCloudinarySignature(folderName: string) {
    return this.http.get<any>(
      `${apiBaseUrl}/Cloud/signature?folderName=${folderName}`,
    );
  }

  uploadToCloudinary(file: File, sigData: any) {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('signature', sigData.signature);
    formData.append('timestamp', sigData.timestamp);
    formData.append('api_key', sigData.apiKey);
    formData.append('folder', sigData.folder);

    const cloudinaryUrl = `https://api.cloudinary.com/v1_1/${sigData.cloudName}/image/upload`;

    return this.http.post(cloudinaryUrl, formData, {
      reportProgress: true,
      observe: 'events',
    });
  }
}
