export interface PlugInConfiguration {
    readonly id: any;
    readonly gameConfigurationId: any;
    readonly typeId: any;
    readonly isActive: boolean;
    readonly typeName: string;
    readonly plugInName: string;
    readonly plugInDescription: string;
    readonly plugInPointName: string;
    readonly plugInPointDescription: string;
    readonly customPlugInSource: string;
    readonly externalAssemblyName: string;    
}