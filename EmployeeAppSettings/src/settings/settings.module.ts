import { Module } from '@nestjs/common';
import { PrismaModule } from 'prisma/prisma.module';
import { SettingsService } from './settings.service';
import { PrismaService } from 'prisma/prisma.service';
import { SettingsController } from './settings.controller';

@Module({
  imports: [PrismaModule],
  controllers: [SettingsController],
  providers: [SettingsService, PrismaService],
})
export class SettingsModule {}
