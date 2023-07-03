
<template>
	<Suspense>
		<component :is="$route.meta.layout || 'div'"></component>
	</Suspense>
</template>
