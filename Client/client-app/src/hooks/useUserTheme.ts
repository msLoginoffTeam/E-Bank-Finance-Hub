import {useUserQuery} from "../queries/accounts.queries.ts";
import {useCreateThemeMutation, useSetThemeMutation, useThemeQuery} from "../queries/theme.queries.ts";
import {useLocalStorage} from "@mantine/hooks";
import {useMantineColorScheme} from "@mantine/core";
import {useEffect, useState} from "react";
import axios from "axios";

export function useUserTheme() {
    const { data: userId } = useUserQuery();
    const { data: serverRaw, error, isFetched } = useThemeQuery(userId);
    const createTheme = useCreateThemeMutation(userId!, 'Light');
    const setTheme    = useSetThemeMutation(userId!);

    const [lscheme, lset] = useLocalStorage<'light'|'dark'>({
        key: 'mantine-color-scheme', defaultValue: 'light', getInitialValueInEffect: true
    });
    const { colorScheme, setColorScheme } = useMantineColorScheme();

    const [didCreate, setDidCreate] = useState(false);
    useEffect(() => {
        if (
            userId &&
            !didCreate &&
            axios.isAxiosError(error) &&
            error.response?.status === 404
        ) {
            createTheme.mutate(lscheme === "dark" ? "Dark" : "Light");
        }
    }, [userId, error, didCreate, lscheme]);

    useEffect(() => {
        if (
            isFetched &&
            serverRaw &&
            serverRaw.toLowerCase() !== colorScheme
        ) {
            const next = serverRaw.toLowerCase() as 'light'|'dark';
            lset(next);
            setColorScheme(next);
        }
    }, [isFetched, serverRaw]);

    function toggle() {
        const next = colorScheme === 'dark' ? 'light' : 'dark';
        lset(next);
        setColorScheme(next);
        setTheme.mutate(next === 'dark' ? 'Dark' : 'Light');
    }

    return { colorScheme, toggle };
}