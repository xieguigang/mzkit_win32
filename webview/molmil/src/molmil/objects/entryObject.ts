namespace molmil {

    export function simpleEntry() {
        /*
        this.altLocList; // wrap
        this.atomIdList; // not --> index+1
        this.bFactorList; // wrap
        this.bioAssemblyList;
        this.bondAtomList; // probably nowrap
        this.bondOrderList; // probably nowrap
        this.chainIdList; // probably nowrap
        this.chainNameList; // probably nowrap
        this.chainsPerModel; // required???
        this.depositionDate; // not
        this.entityList; // required???
        this.experimentalMethods; // not
        this.groupIdList; // not --> index+1
        this.groupList;
        this.groupTypeList; // wrap-del
        this.groupsPerChain; // required???
        this.insCodeList; // required???
        this.mmtfProducer; //not
        this.mmtfVersion; //not
        this.ncsOperatorList; //not
        this.numAtoms; // wrap-del
        this.numBonds; // wrap-del
        this.numChains;
        this.numGroups;
        this.numModels;
        this.occupancyList;
        this.releaseDate;
        this.secStructList;
        this.sequenceIndexList;
        this.spaceGroup;
        this.structureId;
        this.title;
        this.xCoordList
        this.yCoordList;
        this.zCoordList;
        */

    };
    export class entryObject {
        
        constructor(meta) { // this should become a structure object instead --> models should only be virtual; i.e. only the coordinates should be saved, the structure (chain->residue->atom) is determined by the initial model
            this.chains = [];
            this.meta = meta || {};
            this.display = true;
            this.programs = [];
            this.BUmatrices = {};
            this.BUassemblies = {};
        };


        toString() { return "Entry " + (this.meta.id || ""); };

    }
}