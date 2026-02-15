import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AiEnvironment } from '../utils/app.config';
import { map, Observable } from 'rxjs';
import { GoogleGenerativeAI } from '@google/generative-ai';

@Injectable({
  providedIn: 'root',
})
export class AiService {
  private genAI = new GoogleGenerativeAI(AiEnvironment.apiKey);
  private model = this.genAI.getGenerativeModel({
    model: 'gemini-3-flash-preview',
    generationConfig: {
      responseMimeType: 'application/json', // ده بيجبر الموديل يطلع JSON بس
    },
  });

  async extractIdData(file: File) {
    // 1. تحويل الصورة لـ Base64
    const base64Data = await this.fileToGenerativePart(file);

    const prompt = `
      Extract the following from this Egyptian ID image in JSON format:
      - full_name (الاسم بالكامل)
      - identity_number (الرقم القومي - 14 digits)
      - birth_date (تاريخ الميلاد من الرقم القومي)
      - address (العنوان)
      Return ONLY the JSON object.
    `;

    try {
      const result = await this.model.generateContent([prompt, base64Data]);
      const response = await result.response;
      const text = response.text();

      // تنظيف الـ JSON من أي كلام زيادة
      const jsonString = text.replace(/```json|```/g, '');
      return JSON.parse(jsonString);
    } catch (error) {
      console.error('Gemini Error:', error);
      throw error;
    }
  }

  // Helper function لتحويل الملف لـ format بيفهمه Gemini
  private async fileToGenerativePart(file: File): Promise<any> {
    const base64Promise = new Promise((resolve) => {
      const reader = new FileReader();
      reader.onloadend = () => resolve((reader.result as string).split(',')[1]);
      reader.readAsDataURL(file);
    });

    return {
      inlineData: { data: await base64Promise, mimeType: file.type },
    };
  }
}
