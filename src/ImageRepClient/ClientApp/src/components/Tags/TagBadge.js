import React from 'react'

function TagBadge(props) {
    return (
        <span key={props.Tag.tagId} className="badge badge-pill badge-secondary tag-badge-pill m-1">
            {props.Tag.name}&nbsp;
            {
                props.deleteButton?(
                    <a href="#" className="text-reset text-decoration-none" onClick={(e) => props.handleRemoveTag(e, props.Tag.tagId)}>X</a>
                ):
                (
                    <></>
                )
            }
        </span>
        );
}
export default TagBadge;