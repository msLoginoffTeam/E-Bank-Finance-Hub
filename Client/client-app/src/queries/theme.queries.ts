import { useQuery, useMutation } from "@tanstack/react-query";
import { queryKeys }            from "./queryKeys";
import { invalidateTheme }      from "./invalidateQueries";
import { ThemeAPI }             from "../services/theme.service";

export const useThemeQuery = (userId: string) => {
    return useQuery({
        queryKey:   queryKeys.theme(userId),
        queryFn:    () => ThemeAPI.getTheme(userId),
        retry: false,
        enabled: !!userId,
        refetchOnWindowFocus: false
    });
};

export const useCreateThemeMutation = (userId: string, initial: "Light" | "Dark") => {
    return useMutation({
        mutationFn: (theme: "Light"|"Dark") => ThemeAPI.createTheme(userId, theme),
        onSuccess:  () => invalidateTheme(userId),
    });
};

export const useSetThemeMutation = (userId: string) => {
    return useMutation({
        mutationFn: (theme: "Light" | "Dark") => ThemeAPI.setTheme(userId, theme),
        onSuccess:  () => invalidateTheme(userId),
    });
};
