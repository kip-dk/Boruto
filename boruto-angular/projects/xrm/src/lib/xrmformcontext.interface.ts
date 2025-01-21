
export interface XrmFormContext {
    Navigation: Navigation;
    EnkeltProveoprettelse: EnkeltProveoprettelse | null;
}

export interface Navigation {
    navigateTo(input: PageInputEntityRecord): Promise<any>;
}

export interface PageInputEntityRecord {
    pageType: "entityrecord" | "entitylist";
    entityName: string;
    entityId: string;
}

interface EnkeltProveoprettelse {
    setAntalValgte(v: number): void;
}