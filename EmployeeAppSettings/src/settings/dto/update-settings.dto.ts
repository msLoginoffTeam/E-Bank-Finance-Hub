import { ApiProperty } from '@nestjs/swagger';
import { IsArray, IsEnum, IsOptional, IsString } from 'class-validator';

enum Theme {
  LIGHT = 'LIGHT',
  DARK = 'DARK',
}

export class UpdateSettingsDto {
  @ApiProperty({ enum: ['LIGHT', 'DARK'] })
  @IsEnum(Theme)
  @IsOptional()
  theme?: Theme;

  @ApiProperty()
  @IsArray()
  @IsString({ each: true })
  @IsOptional()
  hiddenAccounts?: string[];
}
