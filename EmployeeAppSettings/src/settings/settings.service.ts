import { Injectable } from '@nestjs/common';
import { UserSettings } from '@prisma/client';
import { PrismaService } from 'prisma/prisma.service';
import { UpdateSettingsDto } from './dto/update-settings.dto';

@Injectable()
export class SettingsService {
  constructor(private prisma: PrismaService) {}

  async getSettings(userId: string): Promise<UserSettings> {
    let settings: UserSettings | null =
      await this.prisma.userSettings.findUnique({
        where: { userId },
      });

    if (!settings) {
      settings = await this.prisma.userSettings.create({
        data: {
          userId,
          theme: 'LIGHT',
          hiddenAccounts: [],
        },
      });
    }

    return settings;
  }

  async updateSettings(userId: string, updateSettingsDto: UpdateSettingsDto) {
    return this.prisma.userSettings.upsert({
      where: { userId },
      update: {
        theme: updateSettingsDto.theme,
        hiddenAccounts: updateSettingsDto.hiddenAccounts,
      },
      create: {
        userId,
        theme: updateSettingsDto.theme || 'LIGHT',
        hiddenAccounts: updateSettingsDto.hiddenAccounts || [],
      },
    });
  }
}
