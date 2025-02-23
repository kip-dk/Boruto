
export interface XrmRoot {
    Navigation: Navigation;
}

export interface Navigation {
    navigateTo(input: PageInputEntityRecord): Promise<any>;
}

export interface PageInputEntityRecord {
    pageType: "entityrecord" | "entitylist";
    entityName: string;
    entityId: string;
}
