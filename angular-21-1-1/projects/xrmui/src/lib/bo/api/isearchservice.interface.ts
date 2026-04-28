import { ISelectable } from './iselectable.interface';

export interface ISearchService {
  search(v: string):Promise<ISelectable[]>;
}
