export interface ChangedNotifier {
    notifyOnChanged(): void
    onChanged?(): void;
}