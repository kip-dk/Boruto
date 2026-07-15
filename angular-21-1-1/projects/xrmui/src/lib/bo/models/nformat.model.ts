const formatters = new Map<number, Intl.NumberFormat>();

export const nFormater = {
    localizer: "da-DK",
    format(value: number, decimals: number): string {

         let formatter = formatters.get(decimals);

         if (!formatter) {
            formatter = new Intl.NumberFormat(this.localizer, {
            minimumFractionDigits: decimals,
            maximumFractionDigits: decimals,
            });

            formatters.set(decimals, formatter);
         }
        return formatter.format(value);
    },
    flex(value: number): string {
        let formatter = formatters.get(9999);

         if (!formatter) {
            formatter = new Intl.NumberFormat(this.localizer, {
            minimumFractionDigits: 0,
            maximumFractionDigits: 10});

            formatters.set(9999, formatter);
         }
        return formatter.format(value);
    }
 };