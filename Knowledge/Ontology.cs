#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian2/Ontology.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Knowledge {
    public class Ontology {
        //wikipedia says this about Ontologies:
        // http://wikipedia.org/wiki/Ontology_(information_science)
        /*
         * an ontology is a formal representation of knowledge as a set of concepts within a domain, and the relationships between those concepts
         */

        /* right now, RDF (owl?) files are hard coded with the subClasses.
         * we need an ability to make the values Truthier or Falsier
         * but most likely leave the values connected (not deleted)
         * 
         * as the engine/logic/whatever determines better about the data.
         * 
         * 
         * Our AI is not going to think/live in (or from) these ontologies, these are just for storing information that it has learned.
         * ie, the relationships between things.
         * 

        <domain id="word" >
          <subClassOf id="anotherword" >
        </domain>
        */
    }
}
