import { MantineProvider, useMantineColorScheme, ColorSchemeScript, createTheme } from '@mantine/core';
import { useLocalStorage } from '@mantine/hooks';
import '@mantine/core/styles.css';
import './global.css';
import {useUserTheme} from "./hooks/useUserTheme.ts";
import {AppRouter} from "./router";

const themeOverrides = createTheme({
    primaryColor: 'blue',
    fontFamily: 'Inter, sans-serif',
});

export function App() {
    const [scheme, setScheme] = useLocalStorage<'light'|'dark'>({
        key: 'mantine-color-scheme',
        defaultValue: 'light',
        getInitialValueInEffect: true,
    });
    return (
        <>
            <ColorSchemeScript defaultColorScheme="light" />

            <MantineProvider
                defaultColorScheme={scheme}
                colorSchemeManager={{
                    get:    () => scheme,
                    set:    (val) => setScheme(val === 'dark' ? 'dark' : 'light'),
                    subscribe:    () => {},
                    unsubscribe:  () => {},
                    clear:  () => setScheme('light'),
                }}
                theme={themeOverrides}
            >
                <AppRouter />
                <AppContent />
            </MantineProvider>
        </>
    );
}

function AppContent() {
    const { colorScheme, toggle } = useUserTheme();

    return (
        <>
            <button onClick={toggle}>
                {colorScheme === 'dark' ? 'Светлая тема' : 'Тёмная тема'}
            </button>
        </>
    );
}

export default App;