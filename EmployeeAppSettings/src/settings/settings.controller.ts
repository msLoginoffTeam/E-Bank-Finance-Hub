import { Body, Controller, Get, Param, Put } from '@nestjs/common';
import { SettingsService } from './settings.service';
import { ApiTags } from '@nestjs/swagger';
import { UpdateSettingsDto } from './dto/update-settings.dto';

@ApiTags('settings')
@Controller('settings')
export class SettingsController {
  constructor(private readonly settingsService: SettingsService) {}

  @Get(':userId')
  async getSettings(@Param('userId') userId: string) {
    return this.settingsService.getSettings(userId);
  }

  @Put(':userId')
  async updateSettings(
    @Param('userId') userId: string,
    @Body() updateSettingsDto: UpdateSettingsDto,
  ) {
    return this.settingsService.updateSettings(userId, updateSettingsDto);
  }
}
